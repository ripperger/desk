using Desk.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Desk.Models.Device.MultiFunctionPrinter
{
    public class KonicaMinolta : Printer
    {
        /// <summary>
        /// From a SelectList
        /// </summary>
        [Required(ErrorMessage = Messages.Required)]
        [Display(Name = "Szervízkód")]
        public int DeviceId { get; set; }

        [Required(ErrorMessage = Messages.Required)]
        [Display(Name = "Szkennelt oldalak száma")]
        [Range(0, int.MaxValue, ErrorMessage = Messages.RangeNotNegative)]
        public int ScannedCount { get; set; }

        [Display(Name = "Megelőző karbantartás szükséges")]
        public bool IsPreventiveMaintenance { get; set; }

    }

    public class KonicaMinoltaViewModel : SpecialPropertiesViewModel
    {
        public new KonicaMinolta? SpecialProperties { get; set; }
        public KonicaMinoltaViewModel() : base() {
            SpecialProperties = new KonicaMinolta();
        }

    }
}
