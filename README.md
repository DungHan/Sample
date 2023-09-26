# 這不是一個可執行的專案，僅為了展示寫 Code 的風格，內容非常精簡

專案的基礎架構用本公司架構師撰寫的 MDP 來開發，該框架包裝了一些核心工具，並規範 DDD 風格讓開發者遵循
https://github.com/Clark159/MDP.Net

下面簡單說明各專案用途
1. Sinyi.SuperAgent.WebApp 啟動專案，內含 Hosting.json 提供給底層解析從 DI 容器解析指定 repository 注入 domain。
2. Sinyi.Customers，Customer domain 的 context，在此定義 repository 與 entity，並提供 context 給外界使用。
3. Sinyi.Customers.Accesses，存取層，再此會用裝飾者模式疊加快取或其他資料處理，如資料加密等，是否要用快取可以從 Hosting.json 進行設定。
4. Sinyi.Customers.Hosting，設定注入到 DI 容器的 repository 如何解析其實例。
5. Sinyi.Customers.Services，實作 WebApi
6. Sinyi.Caching，快取專案
