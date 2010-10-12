using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MemBus;

namespace Membus.WpfTwitterClient.GatherAccessToken
{
    public class ScanContentForVerifierHandler : Handles<RequestToScanContentForVerifier>
    {
        private static readonly Regex verifier = new Regex(@"\d\d\d\d\d\d\d", RegexOptions.Multiline | RegexOptions.Compiled);

        protected override void push(RequestToScanContentForVerifier message)
        {
            var match = verifier.Match(message.Content);
            if (match.Captures.Count == 0)
                return;
            var possibleVerifier = match.Captures[0].Value;
            Debug.WriteLine("Found a verifier: " + possibleVerifier);
        }
    }
}