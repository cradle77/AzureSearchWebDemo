namespace AdventureWorksWeb.Models
{
    public class PagingInfo
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int Skip
        {
            get { return this.CurrentPage * PageSize; }
        }

        public int Top
        {
            get { return this.PageSize; }
        }

        public PagingInfo()
        {
            this.CurrentPage = 0;
            this.PageSize = 20;
        }

        public override string ToString()
        {
            return this.CurrentPage.ToString();
        }
    }
}