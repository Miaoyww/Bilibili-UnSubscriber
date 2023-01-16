namespace Bilibili_UnSubscribe.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}