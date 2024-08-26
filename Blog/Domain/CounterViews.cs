namespace Blog.Domain;

public class CounterViews
{
    public Guid ArticleId { get; set; }
    public Article Article { get; set; }
    public int Counter { get; set; }

    public void IncrementsArticleView()
    {
        Counter++;
    }
}
