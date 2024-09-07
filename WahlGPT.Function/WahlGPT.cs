using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.DataFormats.Text;
using WahlGPT.Common;

namespace WahlGPT.Function;

public class WahlGPT
{
	IKernelMemory _kernelMemory;


	[FunctionName("WahlGPT")]
	public async Task<IActionResult> Run(
		[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
		ILogger log)
	{
		try
		{
			_kernelMemory = Settings.BuildKernelMemory();
			var formdata = await req.ReadFormAsync();

			string question = formdata["question"];
			string documentIds = formdata["documentIds"];
			string userId = formdata["userId"];

			var filters = new List<MemoryFilter>();
			foreach (var documentId in documentIds.Split(','))
			{
				var memoryFilter = new MemoryFilter();
				memoryFilter = memoryFilter.ByDocument(documentId);
				filters.Add(memoryFilter);
			}

			question += " - Bitte kurz und prägnant antworten";
			var result = await _kernelMemory.AskAsync(question, filters: filters);
			return new OkObjectResult(result);
		}
		catch (Exception ex)
		{
			log.LogError(ex, "Error in AskQuestion " + ex);
			return new BadRequestObjectResult("Error in AskQuestion " + ex);
		}
	}
}
