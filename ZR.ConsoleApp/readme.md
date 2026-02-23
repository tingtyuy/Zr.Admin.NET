## 安装环境
$env:PLAYWRIGHT_DOWNLOAD_HOST="https://npmmirror.com/mirrors/playwright"
pwsh bin/Debug/net10.0/playwright.ps1 install
## 打开codegen
pwsh bin/Debug/net10.0/playwright.ps1 codegen https://www.doubao.com/chat --channel msedge
pwsh bin/Debug/net10.0/playwright.ps1 codegen https://www.doubao.com/chat --channel msedge --load-storage=bin/Debug/net10.0/auth.json
pwsh bin/Debug/net10.0/playwright.ps1 codegen https://www.doubao.com/chat --channel msedge --save-storage=bin/Debug/net10.0/auth.json