using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Blog.Domain;

public class CounterViews
{
    [Key]
    public Article Article { get; set; }
    public Guid ArticleId { get; set; }
    public int Counter { get; set; }

    public void IncrementsArticleView()
    {
        Counter++;
    }
}
