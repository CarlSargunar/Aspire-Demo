using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.Models;
public class PageView
{
    [Key]
    public int Id { get; set; }
    public string URL { get; set; }
    public DateTime LastAccessed { get; set; }
    public int Count { get; set; }
}
