// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;

namespace DrunkAudible.Data.Models
{
    public interface IManMadeItem
    {
        int Id { get; }

        String Title { get; }

        Author[] Authors { get; }

        String Narrator { get; }
    }
}

