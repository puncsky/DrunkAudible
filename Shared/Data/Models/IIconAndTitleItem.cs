// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;

namespace DrunkAudible.Data.Models
{
    public interface IIconAndTitleItem
    {
        int Id { get; }

        String Title { get; }

        String IconUrl { get; }
    }
}

