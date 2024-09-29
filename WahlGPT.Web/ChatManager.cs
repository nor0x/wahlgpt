using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.KernelMemory;
using WahlGPT.Common;
using static System.Net.WebRequestMethods;

namespace WahlGPT;

public class ChatManager
{
	public Dictionary<string, string> _questionCache = new();
	public ChatManager(HttpClient client, IJSRuntime jsRuntime)
	{
		_client = client;
		_jsRuntime = jsRuntime;
	}


	readonly HttpClient _client;
	readonly IJSRuntime _jsRuntime;

	public async Task<int> GetCount()
	{
		return 26237;
		var lastDate = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "countDate");
		if (!string.IsNullOrWhiteSpace(lastDate))
		{
			var lastDateTime = DateTime.Parse(lastDate);
			if (DateTime.Now.Subtract(lastDateTime).TotalMinutes < 10)
			{
				var lastCount = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "count");
				if (!string.IsNullOrWhiteSpace(lastCount))
				{
					var minutes = DateTime.Now.Subtract(lastDateTime).TotalMinutes;
					var estimatedCount = (int)(minutes * 10) + int.Parse(lastCount);
					return estimatedCount;
				}
			}
		}

		var countString = await _client.GetStringAsync(Settings.CountUrl);
		await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "count", countString);
		await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "countDate", DateTime.Now.ToString());
		return int.Parse(countString);
	}

	public async Task<MyMemoryAnswer?> AskQuestion(string question, List<string> documentIds)
	{
		try
		{
			foreach (var docId in documentIds)
			{
				var key = $"{docId}_{question.ToLowerInvariant()}";
				if (_questionCache.ContainsKey(key))
				{
					var answer = _questionCache[key];
					var cachedAnswer = new MyMemoryAnswer();
					cachedAnswer = cachedAnswer.FromJson(answer);
					return cachedAnswer;
				}
			}

			var docIds = string.Join(",", documentIds);
			var request = new HttpRequestMessage();
			request.RequestUri = new Uri(Settings.ApiEndpoint);
			request.Method = HttpMethod.Post;


			var content = new MultipartFormDataContent();
			content.Add(new StringContent(question), "Question");
			content.Add(new StringContent("0"), "UserId");
			content.Add(new StringContent(docIds), "DocumentIds");
			request.Content = content;

			var response = await _client.SendAsync(request);
			var result = await response.Content.ReadAsStringAsync();
			var memoryAnswer = new MyMemoryAnswer();
			memoryAnswer = memoryAnswer.FromJson(result);

			foreach (var docId in documentIds)
			{
				var key = $"{docId}_{question.ToLowerInvariant()}";
				_questionCache[key] = result;
			}

			return memoryAnswer;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return null;
		}
	}

	public static string ConvertToHtml(string inputText)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(inputText))
				return string.Empty;

			// Convert **text** to <strong>text</strong>
			string html = Regex.Replace(inputText, @"\*\*(.*?)\*\*", "<strong>$1</strong>");

			// Replace line breaks
			html = Regex.Replace(html, @"\n\n", "<br><br>");
			html = Regex.Replace(html, @"\n", "<br>");

			// Convert lists: naive approach assuming all lines that start with "- " are list items
			// Begin list
			html = Regex.Replace(html, @"<br>\s*-\s", m =>
			{
				if (m.Index == 0 || html[m.Index - 1] == '>')
					return "<ul><li>";
				else
					return "</li><li>";
			}, RegexOptions.Singleline);

			// End list
			html = html + "</li></ul>";

			// Clean up: Replace "<ul></li>" occurrences which appear just before a list starts
			html = Regex.Replace(html, @"<ul></li>", "<ul>");

			return html;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return inputText;
		}
	}
}
