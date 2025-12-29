# ETF Portfolio Analyzer (ETF 持股變動比對系統)

[下載 Releases](https://github.com/alexshao0807/ETF-Analyzer/releases) | [技術堆疊](#技術堆疊) | [使用說明](#使用說明) | [開發紀錄](#開發紀錄) | [作者](#作者)

ETF Portfolio Analyzer 是一個針對 Windows 環境開發的 ETF 成分股比對工具。本專案核心目標在於透過高效能演算法，解決分析師在處理每日成分股權重異動時，因人工比對 Excel 所導致的低效與誤差問題。

---

## 項目定位與邏輯

在量化分析中，成分股的微量調倉往往隱含法人的交易意圖。本工具不進行複雜的預測，而是專注於「數據分類與邊界界定」。透過 $O(1)$ 的雜湊比對邏輯，將兩份時間序列的數據進行精確差分，並將結果結構化輸出。

### 核心特性

* 效能優化：核心演算法基於 Hash-based Dictionary，處理萬級數據維持常數時間複雜度，避開 $O(N^2)$ 的效能瓶頸。
* 非同步機制：採用 .NET Async/Await 模式處理 I/O 密集型任務，確保 Excel 讀取期間 UI 執行緒不阻塞。
* 自動化報表：比對後自動生成帶有時間戳記的 .txt 報告，支援自動導引至輸出目錄。
* 健壯性設計：內置 Log 系統與 Exception Handling，涵蓋路徑檢查與非法格式防呆。

---

## 技術堆疊

* Runtime: .NET Framework 4.7.2
* Language: C#
* Library: 
    * ClosedXML: OpenXML 封裝庫，處理 Excel XML 數據。
    * System.Threading.Tasks: 異步並發控制。
* Build Tool: Visual Studio 2022

---

## 安裝與執行

1. 於本專案 Releases 處下載 ETF_Analyzer_v2.0.zip。
2. 解壓縮至本地目錄。
3. 執行 ETF_Compare.exe。

註：由於未進行數位簽章，Windows Defender 若出現警告，請點選「其他資訊」並選擇「仍要執行」。

---

## 使用說明

1. 數據導入：將昨日與今日的成分股 Excel 檔案拖入程式介面或手動選取路徑。
2. 分析比對：點擊「比對」按鈕，程式將進行差分運算（含新增、股數變動）。
3. 結果導出：運算完成後會生成報告資料夾，內含詳細變動明細。
4. 清理：點擊「清除」按鈕重置文字框內容。

---

## 開發紀錄

### v2.0.0
- 新增：Drag & Drop 檔案拖放支援。
- 新增：Log 日誌，記錄執行過程與非預期錯誤。

### v1.0.0
- 實作 O(1) 比對算法。
- 完成 Release 模式封裝與打包。

---

## 免責聲明

* 本項目僅供技術交流與數據研究使用，不構成任何投資建議。
* 投資者應自行負擔損益，作者不對使用本軟體產生的任何損失負責。
* 數據計算結果僅供參考，請以各 ETF 發行官方公佈之數據為準。

---

## 作者

Alex Hao
- GitHub: [@alexshao0807](https://github.com/alexshao0807)
