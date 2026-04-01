# RevitAddinHotReloadDemo 架構

## 1. 整體架構

本專案採用 **Host + Logic 分離架構**，讓 Revit Add-in 可以做到類似 Hot Reload 的開發流程。

```
Revit
   ↓
RevitAddinHost.dll   (固定 Host)
   ↓
Loader.cs
   ↓
dist/RevitLogic.dll  (可替換 Logic)
```

專案結構：

```
RevitAddinHotReloadDemo
│
├─ dist
│   RevitLogic.dll
│   RevitLogic.pdb
│
├─ RevitAddinHost
│   App.cs
│   CommandButton1.cs
│   CommandButton2.cs
│   Loader.cs
│   RevitAddinHost.csproj
│
└─ RevitLogic
    Entry.cs
    RevitLogic.csproj
```

---

# 2. 各專案角色

## RevitAddinHost (Host)

這個 DLL 會被 Revit 直接載入。

負責：

- 建立 Ribbon UI
- 按鈕 Command
- 載入 Logic DLL

主要檔案：

```
App.cs
CommandButton1.cs
CommandButton2.cs
Loader.cs
```

Revit 啟動時：

```
Revit
 ↓
Load RevitAddinHost.dll
 ↓
App.OnStartup()
 ↓
建立 Ribbon
```

---

## RevitLogic (Logic)

這是 **真正的功能程式碼**。

例如：

- 自動標註
- BOP 計算
- 套管生成
- Dynamo 轉 C# 功能

主要入口：

```
Entry.cs
```

例如：

```
namespace RevitLogic
{
    public class Entry
    {
        public string Run()
        {
            return "Hello from Logic";
        }
    }
}
```

---

# 3. Loader 的作用

Loader 會在執行時載入 `dist/RevitLogic.dll`。

```
byte[] asmBytes = File.ReadAllBytes(LogicDllPath);
Assembly asm = Assembly.Load(asmBytes);
```

這樣做的原因：

- 避免 Revit 鎖住 DLL
- 可以覆蓋 dist DLL
- 支援 Hot Reload

流程：

```
Revit
 ↓
CommandButton
 ↓
Loader.Call()
 ↓
Load dist/RevitLogic.dll
 ↓
Execute Entry.Run()
```

---

# 4. 為什麼要用 dist 資料夾

RevitLogic build 出來的 DLL 在：

```
RevitLogic/bin/Debug/RevitLogic.dll
```

但 Revit 執行時讀的是：

```
dist/RevitLogic.dll
```

所以 build 後需要 copy。

---

# 5. 日常開發流程 (Hot Reload)

修改：

```
RevitLogic/Entry.cs
```

只需要：

```
dotnet build .\RevitLogic\RevitLogic.csproj

Copy-Item .\RevitLogic\bin\Debug\RevitLogic.dll .\dist\RevitLogic.dll -Force
Copy-Item .\RevitLogic\bin\Debug\RevitLogic.pdb .\dist\RevitLogic.pdb -Force
```

然後：

```
回 Revit
按 Ribbon 按鈕
```

新的 Logic 就會被載入。

不需要：

- 重開 Revit
- rebuild Host

---

# 6. 什麼時候需要重開 Revit

判斷規則：

| 修改檔案          | 是否需要重開 Revit |
| ----------------- | ------------------ |
| RevitLogic/\*     | 不需要             |
| RevitAddinHost/\* | 需要               |

原因：

Revit 啟動時會載入：

```
RevitAddinHost.dll
```

而 .NET Framework：

```
Assembly 無法卸載
```

所以 Host DLL 會被鎖住。

---

# 7. 正確開發節奏

第一次啟動前：

```
dotnet build .\RevitAddinHost\RevitAddinHost.csproj
```

之後整天：

只 build Logic。

```
dotnet build .\RevitLogic\RevitLogic.csproj

powershell -ExecutionPolicy Bypass -File .\build.ps1

powershell -ExecutionPolicy Bypass -File .\release.ps1

powershell -ExecutionPolicy Bypass -File .\release.ps1 -Version 0.1.0
```

---

# 8. 建議建立 build script

建立：

```
build.ps1
```

內容：

```
dotnet build .\RevitLogic\RevitLogic.csproj -v minimal

Copy-Item .\RevitLogic\bin\Debug\RevitLogic.dll .\dist\RevitLogic.dll -Force
Copy-Item .\RevitLogic\bin\Debug\RevitLogic.pdb .\dist\RevitLogic.pdb -Force

Write-Host "RevitLogic updated"
```

使用：

```
.\build.ps1
```

更新 Logic DLL。

---

# 9. 為什麼要用 Host + Logic 架構

Revit Add-in 最大問題：

```
Revit 不支援 Hot Reload
```

DLL 被鎖住。

解法：

```
Host (固定)
Logic (可替換)
```

這樣：

```
改程式
build
copy
按按鈕
```

就能更新。

---

# 10. 未來可擴充

這個架構可以升級成：

- 多 Command Routing
- 自動 build script
- DLL 版本檢查
- 動態 plugin system

適合：

- 大型 Revit Add-in
- Dynamo → C# 轉換專案
- BIM 自動化工具

# 11. Revit 外掛的註冊清單（.addin manifest）。

Revit 啟動時會去讀XML，知道「要載入哪個 DLL、從哪個類別開始執行」。沒有它，Revit 不知道你的外掛存在。

產生新的 GUID 在PowerShell 跑：
```
[guid]::NewGuid().ToString().ToUpper()
```


興建檔案: C:\Users\AppData\Roaming\Autodesk\Revit\Addins\2024\RevitAddinHotReloadDemo.addin
'''

<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>
  <AddIn Type="Application">
    <Name>RevitAddinHotReloadDemo</Name>
    <Assembly>D:\Project\BIM+C#\RevitAddinHotReloadDemo\RevitAddinHost\bin\Debug\RevitAddinHost.dll</Assembly>
    <AddInId>新的 GUID</AddInId>
    <FullClassName>RevitAddinHost.App</FullClassName>
    <VendorId>TEST</VendorId>
    <VendorDescription>RevitAddinHotReloadDemo</VendorDescription>
  </AddIn>
</RevitAddIns>
'''
