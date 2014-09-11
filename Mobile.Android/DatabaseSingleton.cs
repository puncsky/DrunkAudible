// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System.Linq;
using DrunkAudible.Data;
using DrunkAudible.Data.Models;

namespace Mobile.Android
{
    public static class DatabaseSingleton
    {
        static ObjectRelationalMapping _orm;

        public static ObjectRelationalMapping Orm
        {
            get
            {
                if (_orm == null)
                {
                    _orm = new ObjectRelationalMapping ();
                    if (!_orm.Database.Table<Album> ().Any ())
                    {
                        _orm.InsertOrUpdate (OrmInitializer.AlbumSamples);
                    }
                }
                return _orm;
            }
        }
    }
}

