﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpecialAbility
{
    Currency GetCost();

    bool BuyAndActivate();
}
