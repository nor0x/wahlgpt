using Microsoft.KernelMemory;
using WahlGPT.Common;

IKernelMemory? _kernelMemory = null;

try
{
	_kernelMemory = Settings.BuildKernelMemory();

	var root = System.Reflection.Assembly.GetAssembly(typeof(Program)).Location;
	var path = Path.Combine(Path.GetDirectoryName(root), "Documents");

	var files = Directory.GetFiles(path, "*.pdf");
	foreach (var file in files)
	{
		Console.WriteLine("importing " + file);
		var documentId = Path.GetFileNameWithoutExtension(file);
		var importResult = await _kernelMemory.ImportDocumentAsync(file, documentId: documentId);
		Console.WriteLine("imported " + file);
		var docstatus = await _kernelMemory.GetDocumentStatusAsync(documentId);
		Console.WriteLine("completed: " + docstatus.Completed + " failed: " + docstatus.Failed);
	}

	Console.WriteLine("importing done");

	//Test Frage
	var question = "Was ist die Position der ÖVP zur Bildungspolitik?";
	Console.WriteLine("Asking question: " + question);
	var filter = new MemoryFilter();
	filter.ByDocument("oevp");
	var answer = await _kernelMemory.AskAsync(question);
	Console.WriteLine("Answer: " + answer.Result);
}
catch (Exception ex)
{
	Console.WriteLine(ex.Message);
}


