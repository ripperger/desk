using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Desk.Constants;

namespace Desk.Models.Device.MultiFunctionPrinter
{
    public class Printer : SpecialProperties
    {

        /// <summary>
        /// Incident, supplies, both or administration
        /// From SelectList
        /// </summary>
        [Required(ErrorMessage = Messages.Required)]
        [Display(Name = "Bejelentés típusa")]
        public string? RequestType { get; set; }

        [Required(ErrorMessage = Messages.Required)]
        [Display(Name = "Eszköz helye (cím)")]
        [StringLength(60, ErrorMessage = Messages.StringLength60, MinimumLength = 3)]
        public string? Address { get; set; }

        [Required(ErrorMessage = Messages.Required)]
        [Display(Name = "Lapszám - fekete")]
        [Range(0, int.MaxValue, ErrorMessage = Messages.RangeNotNegative)]
        public int BlackCount { get; set; }

        [Required(ErrorMessage = Messages.Required)]
        [Display(Name = "Lapszám - színes")]
        [Range(0, int.MaxValue, ErrorMessage = Messages.RangeNotNegative)]
        public int ColorCount { get; set; }

    }
}
