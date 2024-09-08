# WahlGPT

<img src="https://raw.githubusercontent.com/nor0x/wahlgpt/main/WahlGPT.Web/wwwroot/images/header-img.png" width="250" height="250">
 

[![Deploy to GitHub Pages](https://github.com/nor0x/wahlgpt/actions/workflows/main.yml/badge.svg)](https://github.com/nor0x/wahlgpt/actions/workflows/main.yml) 
[![pages-build-deployment](https://github.com/nor0x/wahlgpt/actions/workflows/pages/pages-build-deployment/badge.svg)](https://github.com/nor0x/wahlgpt/actions/workflows/pages/pages-build-deployment)



**LLM + RAG to generate answers about the program of the parties for the 2024 Austrian legislative election.**


# Dependencies
- .NET SDK >= 8
- wasm workload
- Azure Functions Core Tools

# How to run
```pwsh
# run the backend function
cd WahlGPT.Function
func start

# adapt settings (i.e. API_ENDPOINT from function) in WahlGPT.Common/Settings.cs
# for development the following extensions for KernelMemory can be used:
#.WithSimpleVectorDb()
#.WithSimpleFileStorage()
# with these you don't need a running qdrant instance - the vector db is stored in memory
# feel free to plug in other models for embedding or text generation to experiment with different results

#restore the wasm workloads
cd WahlGPT.Web
dotnet workload restore

# run the frontend
dotnet run
```

## credits
- html5up
- semantic-kernel
- semantic-memory
- qdrant
- dotnet
- openai