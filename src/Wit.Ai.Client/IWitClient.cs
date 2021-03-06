﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Wit.Ai.Client.Types;

namespace Wit.Ai.Client
{
    public interface IWitClient
    {
        Task<TResponse> SendRequestAsync<TResponse>(
            IWitRequest<TResponse> request,
            CancellationToken cancellationToken = default
        );

        Task<Meaning> GetSentenceMeaningAsync(
            string query,
            Context context = default,
            string messageId = default,
            string threadId = default,
            int n = default,
            bool verbose = false,
            CancellationToken cancellationToken = default
        );

        Task<Meaning> GetAudioMeaningAsync(
            Stream audioStream,
            string contentType,
            Context context = default,
            string messageId = default,
            string threadId = default,
            int n = default,
            bool verbose = false,
            CancellationToken cancellationToken = default
        );
    }
}
