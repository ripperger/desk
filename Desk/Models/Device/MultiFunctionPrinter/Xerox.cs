using System.ComponentModel.DataAnnotations;
using Desk.Constants;

namespace Desk.Models.Device.MultiFunctionPrinter
{
    public class Xerox : Printer
    {
        /// <summary>
        /// From a SelectList
        /// </summary>
        [Required(ErrorMessage = Messages.Required)]
        [Display(Name = "Sorozatszám")]
        public int DeviceId { get; set; }

    }
    public class XeroxViewModel : SpecialPropertiesViewModel
    {
        public new Xerox? SpecialProperties { get; set; }
        public XeroxViewModel() : base()
        {
            SpecialProperties = new Xerox();
        }
    }
}
