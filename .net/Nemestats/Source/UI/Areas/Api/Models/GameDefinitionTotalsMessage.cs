﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Areas.Api.Models
{
    public class GameDefinitionTotalsMessage
    {
        public IList<GameDefinitionTotalMessage> SummariesOfGameDefinitionTotals { get; set; }
    }
}
