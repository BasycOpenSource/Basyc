﻿namespace Basyc.Extensions.SignalR.Client.Tests.MethodsServerCanCall;
#pragma warning disable IDE0060 // Remove unused parameter

public class MethodsServerCanCallTexts : IMethodsServerCanCallEmpty, IMethodsServerCanCallText
{
    public string? LastReceivedText { get; private set; }

    public void ReceiveText(string text) => LastReceivedText = text;

    public async Task ReceiveTextAsync(string text)
    {
        LastReceivedText = text;
        await Task.Delay(150);
    }

    public async Task ReceiveTexts(string text, string text2)
    {
        LastReceivedText = text2;
        await Task.Delay(150);
    }

    public async Task ReceiveTexts(string text, string text2, string text3)
    {
        LastReceivedText = text3;
        await Task.Delay(150);
    }
}
