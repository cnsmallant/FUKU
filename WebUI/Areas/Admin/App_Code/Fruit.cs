using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Fruit
{
    //Integer value of a checkbox
    public int Id { get; set; }

    //String name of a checkbox
    public string Name { get; set; }

    //Boolean value to select a checkbox
    //on the list
    public bool IsSelected { get; set; }

    //Object of html tags to be applied
    //to checkbox, e.g.:'new{tagName = "tagValue"}'
    public object Tags { get; set; }

}
