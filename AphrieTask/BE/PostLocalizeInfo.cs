using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AphrieTask.BE
{
    public class PostLocalizeInfo
    {
       public Post postInfo { get; set; }

        public List<LoacalizProperty> localizeInfo { get; set; }

        public List<PostInteraction> postInteractions { get; set; }
    }
}
