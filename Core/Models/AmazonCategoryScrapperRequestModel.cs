namespace Core.Models
{
    public class AmazonCategoryScrapperRequestModel
    {
        public AmazonCategoryScrapperRequestModel()
        {
            SubCategories = new List<AmazonCategoryScrapperRequestModel>();
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public List<AmazonCategoryScrapperRequestModel> SubCategories { get; set; }
    }
}
