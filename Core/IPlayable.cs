﻿/*
 * The MIT License (MIT)
 * Copyright (c) StarX 2017
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyStorm.Core
{
    public interface IPlayable
    {
        bool Update(int currentFrame);
        void Reset();
    }
}
