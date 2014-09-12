// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System.Collections.Generic;
using System;

namespace DrunkAudible.Data.Models
{
	public interface IAudioListViewElement
	{
        int ID { get; }

        String Title { get; }

        IEnumerable<Author> Authors { get; }

        String Narrator { get; }

        String IconUrl { get; }
	}
}

