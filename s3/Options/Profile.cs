using System;
using System.Collections.Generic;
using System.Text;

namespace s3.Options
{
    class Profile : OptionWithParameter<string>
    {

        internal static bool paramIsSet = false;

        protected override bool ParameterIsCompulsory
        {
            get { return false; }
        }

        protected override void ParameterIsSet()
        {
            paramIsSet = true;
        }

    }
}