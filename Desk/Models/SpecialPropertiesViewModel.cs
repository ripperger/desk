namespace Desk.Models
{
    public class SpecialPropertiesViewModel
    {
        public Ticket? Ticket { get; set; }
        public Comment? Comment { get; set; }
        public SpecialProperties? SpecialProperties { get; set; }

        public SpecialPropertiesViewModel()
        {
            Ticket = new Ticket();
            Comment = new Comment();
        }

        //public SpecialProperties? NullCheck()
        //{
        //    return null;
        //}
    }
}
