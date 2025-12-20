# 📈 ETF Portfolio Analyzer (ETF 持股變動比對系統)
一個高效能的 Windows 桌面應用程式，專為投資人與分析師設計。透過自動化演算法，在秒級內比對 ETF 兩日成分股的權重與持股變化，解決人工比對耗時且易錯的痛點。

## ✨ 核心功能 (Key Features)

- **🚀 比對演算法 ($O(1)$)**
  - 採用 Hash-based 資料結構 (Dictionary) 優化搜尋邏輯，即使處理數萬筆成分股資料也能瞬間完成，大幅優於傳統巢狀迴圈 ($O(N^2)$)。
  
- **⚡ 非同步多執行緒 (Async/Await)**
  - 導入非同步設計模式，確保在讀取大型 Excel 檔案或進行複雜運算時，UI 介面依然流暢不卡頓。

- **🖱️ 直覺的操作體驗 (User Experience)**
  - **拖放支援 (Drag & Drop)**：直接將 Excel 檔案拖入視窗即可讀取。
  - **自動化報告**：分析完成後自動生成 `.txt` 變動報表，並建立專屬資料夾管理。
  - **防呆機制**：包含路徑檢查、一鍵清除輸入、以及執行後的自動導引視窗。

- **📊 廣泛的檔案支援**
  - 完整支援 `.xlsx` (Excel) 格式讀取與解析 (基於 ClosedXML)。

## 🛠️ 技術堆疊 (Tech Stack)

- **語言**: C# (.NET Framework 4.7.2)
- **框架**: Windows Forms (WinForms)
- **套件**: 
  - `ClosedXML` (Excel 資料處理)
  - `System.Threading.Tasks` (非同步處理)
- **工具**: Visual Studio 2022, Git

## 📥 安裝與執行 (Installation)

1. 前往本專案的 [**Releases 頁面**](https://github.com/alexshao0807/ETF-Analyzer/releases)。
2. 下載最新的 **`ETF_Analyzer_v1.0.zip`** (請勿只下載 Source code)。
3. 將壓縮檔 **解壓縮** 至桌面或任意資料夾。
4. 雙擊執行 `ETF_Compare.exe`。

> **⚠️ 注意：安全性警告** > 由於本軟體無數位簽章，Windows Defender 可能會跳出藍色警告視窗。  
> **解決方法**：點擊「其他資訊 (More info)」→ 選擇「仍要執行 (Run anyway)」即可。

## 📖 使用說明 (Usage)

1. **載入檔案**：點擊「瀏覽」或直接將昨日/今日的 ETF 持股 Excel 檔拖入對應欄位。
2. **執行分析**：點擊「比對」按鈕。
3. **查看報告**：程式將自動計算新增持股與股數變動，並詢問是否開啟報告資料夾。
4. **重置**：點擊「清除」按鈕即可準備下一次分析。

## 📝 開發紀錄 (Release Notes)

### v1.0.0 - Initial Release
- 完成核心比對功能上線。
- 實作 Release 模式編譯與打包。
- 優化例外處理 (Exception Handling) 與使用者提示訊息。

## 👤 作者 (Author)

**Alex hao**
- GitHub: [@alexshao0807](https://github.com/alexshao0807)

---
*如果你覺得這個專案有幫助，歡迎幫我按一顆 ⭐️ 星星！*
