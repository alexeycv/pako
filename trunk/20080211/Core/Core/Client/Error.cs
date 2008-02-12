using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Client
{
   public enum Error
    {
       ParamsTooMany,
       ParamsNotEnough,
       ParamsWrongType,
       ParamIntExpected,
       ParamStrExpected,
       Syntax
    }
}
