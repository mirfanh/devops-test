using System.Text;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System;
using System.Text.Json;

namespace FileProcessor
{
    public class Category
    {
        [Key]
        public string CategoryName { get; set; }
        public string ParentCategoryName { get; set; }
        public Category(string catBreadCrumb)
        {
            
            Console.WriteLine(catBreadCrumb);
        }
    }
}
