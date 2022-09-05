using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL
{
    
    public class Author
    {
        public User user { get; set; }
    }

    public class Data
    {
        public Repository repository { get; set; }
    }

    public class History
    {
        public List<Node> nodes { get; set; }
    }

    public class Node
    {
        public string oid { get; set; }
        public string messageHeadline { get; set; }
        public Author author { get; set; }
        public DateTime committedDate { get; set; }
        public int additions { get; set; }
        public int deletions { get; set; }
    }

    public class Object
    {
        public string commitUrl { get; set; }
        public string oid { get; set; }
        public History history { get; set; }
    }

    public class Repository
    {
        public string nameWithOwner { get; set; }
        public Object @object { get; set; }
    }

    public class Root
    {
        public Data data { get; set; }
    
    }

    public class User
    {
        public string login { get; set; }
    }







}
