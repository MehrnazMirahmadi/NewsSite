namespace DomainModel.Comon
{
    public class PageModel
    {
        public int PageIndex { get; set; }
        private int pageSize;
        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                if (value == 0)
                {
                    value = 10;
                }
                pageSize = value;
            }

        }
        public int RecordCount { get; set; }
        //private int pageCount;
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    pageSize = 10;
                if (RecordCount % PageSize == 0)
                {
                    return RecordCount / PageSize;
                }
                else
                {
                    return RecordCount / PageSize + 1;
                }
            }
        }
    }
}
