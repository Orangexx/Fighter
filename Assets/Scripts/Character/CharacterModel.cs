using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class CharacterModel:Model
{
    private void Update()
    {
        if(Poise<0)
        {
            Poise += 0.1f;
        }
    }
}
