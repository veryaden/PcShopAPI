# 🏪 PcShopAPI

PcShopAPI 是一套以 **ASP.NET Core Web API** 開發的電子商務後端系統，  
負責處理商城核心功能，包含會員驗證、商品、購物車、訂單、結帳、遊戲點數與廣告等模組。

本專案以「**實務電商系統**」為目標，採用 **三層式架構（Controller / Service / Repository）**，  
並搭配 **JWT 身分驗證、Entity Framework Core、SQL Server** 實作完整後端流程。

---

## 🎯 專案目標

- 建立一套可實際運作的電商後端 API
- 落實乾淨分層、可維護、可擴充的後端架構
- 模擬真實商業邏輯（訂單流程、點數、廣告、會員行為）
- 作為後端工程師求職展示專案

---

## 🚀 主要功能模組

### 🔐 會員與驗證
- JWT 登入驗證機制
- 支援一般登入與第三方登入（Google OAuth）
- Email 驗證流程
- 使用者登入紀錄與安全性設計

### 🛍️ 商品與購物流程
- 商品與 SKU 管理
- 購物車（Cart / CartItems）
- 結帳流程（Checkout）
- 訂單建立、查詢與狀態管理

### 🎮 遊戲與點數系統
- 多款小遊戲分數提交
- Strategy Pattern 設計的點數計算機制
- 遊戲分數轉換為商城點數（GamePoints）
- 點數取得、使用與紀錄追蹤

### 📢 廣告系統
- 廣告資料管理（Ads）
- 廣告位置（Position）與頁面規則
- 點擊紀錄與成效統計
- 後台圖表資料 API（供前端 Chart.js 使用）

### ❓ FAQ 與內容管理
- FAQ 分類與區塊管理
- 支援後台維護與前台顯示

---

## 🧱 系統架構

Controller → Service → Repository → Database

- Controller：API 介面層，處理 Request / Response
- Service：商業邏輯層，集中處理流程與規則
- Repository：資料存取層，負責 EF Core 查詢與操作
- Database：SQL Server（Code First / Fluent API）

專案以 Area 區分模組（Users / Ads / Games / Cart / Orders），  
提升模組化與可維護性。

---

## 🛠️ 使用技術

| 類型 | 技術 |
|------|------|
| 後端框架 | ASP.NET Core Web API |
| 語言 | C# |
| 資料庫 | SQL Server |
| 驗證 | JWT |
| 架構 | 三層式架構 |

