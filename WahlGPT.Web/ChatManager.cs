using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.KernelMemory;
using WahlGPT.Common;

namespace WahlGPT;

public class ChatManager
{
	public ChatManager(HttpClient client)
	{
		_client = client;
	}

	readonly HttpClient _client;
	readonly string _loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

	public async Task<MyMemoryAnswer?> AskQuestion(string question, List<string> documentIds = null)
	{
		try
		{
			var docIds = documentIds != null ? string.Join(",", documentIds) : "";
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
