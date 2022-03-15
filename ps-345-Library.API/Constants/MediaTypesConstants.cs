using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/13/2022 07:18 pm - SSN - [20220313-1902] - [004] - M05-03 - Demo - Supporting vendor-specific media types

namespace Library.API.Constants
{
    public class MediaTypesConstants
    {
        public const string APPLICATION_JSON = "application/json";
        public const string APPLICATION_XML = "application/xml";
        public const string APPLICATION_VND_MARVIN_BOOK_JSON = "application/vnd.marvin.book+json";
        public const string APPLICATION_VND_MARVIN_BookWithConcatenatedAuthorName_JSON = "application/vnd.marvin.bookwithconcatenatedauthorname+json";

        // 03/14/2022 01:23 am - SSN - [20220314-0111] - [003] - M05-08 - Demo - Supporting schema variation by media type (Input)
        public const string APPLICATION_VND_MARVIN_BookForCreation_JSON = "application/vnd.marvin.bookforcreation+json";
        public const string APPLICATION_VND_MARVIN_BookForCreationWithAmountOfPages_JSON = "application/vnd.marvin.bookforcreationwithamountofpages+json";

    }
}
