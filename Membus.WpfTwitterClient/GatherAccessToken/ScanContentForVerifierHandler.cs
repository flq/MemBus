using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MemBus;
using Membus.WpfTwitterClient.Frame.UI;

namespace Membus.WpfTwitterClient.GatherAccessToken
{
    public class ScanContentForVerifierHandler : Handles<RequestToScanContentForVerifier>
    {
        private readonly IBus bus;
        private static readonly Regex verifier = new Regex(@"\d\d\d\d\d\d\d", RegexOptions.Multiline | RegexOptions.Compiled);

        public ScanContentForVerifierHandler(IBus bus)
        {
            this.bus = bus;
        }

        protected override void push(RequestToScanContentForVerifier message)
        {
            var match = verifier.Match(message.Content);
            if (match.Captures.Count == 0)
                return;
            var possibleVerifier = match.Captures[0].Value;

            bus.Publish(new RequestForAttention(new VerifyPin(possibleVerifier)));
        }
    }
}