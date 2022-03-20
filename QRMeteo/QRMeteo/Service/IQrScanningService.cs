using System.Threading.Tasks;

namespace QRMeteo.Service
{
    public interface IQrScanningService
    {
        Task<string> ScanAsync();
    }
}
