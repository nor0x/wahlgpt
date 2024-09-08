﻿using System.Text.Encodings.Web;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.DataFormats.Text;

namespace WahlGPT.Common;

public static class Settings
{
	public const string ApiEndpoint = "API_ENDPOINT";
	public const string OpenAiApiKey = "OPENAI_API_KEY";
	public const string EmbeddingModel = "text-embedding-3-small";
	public const string TextModel = "gpt-4o";
	public const string QdrantHost = "QDRANT_HOST";
	public const string QdrantApiKey = "QDRANT_API_KEY";
	public const string BlobConnectionString = "BLOB_CONNECTION_STRING";

	static IKernelMemory? _kernelMemory;


	public static IKernelMemory BuildKernelMemory()
	{
		try
		{
			if (_kernelMemory is null)
			{
				var embeddingConfig = new OpenAIConfig
				{
					APIKey = OpenAiApiKey,
					EmbeddingModel = EmbeddingModel,
					TextModel = TextModel,
				};

				var generationConfig = new OpenAIConfig
				{
					APIKey = OpenAiApiKey,
					TextModel = TextModel,
				};

#pragma warning disable KMEXP00 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
				var kernelMemory = new KernelMemoryBuilder()
					.WithContentDecoder(new TextDecoder())
					  .WithOpenAITextEmbeddingGeneration(embeddingConfig)
					  .WithOpenAITextGeneration(generationConfig)
					  .WithSearchClientConfig(new SearchClientConfig
					  {
						  Temperature = 0.7,
						  EmptyAnswer = "Auf diese Frage habe ich leider keine Antwort? 🤔"
					  })
					  .WithQdrantMemoryDb(QdrantHost, QdrantApiKey)
					  .Build();
#pragma warning restore KMEXP00 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

				_kernelMemory = kernelMemory;
			}
			return _kernelMemory;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}
	}
}