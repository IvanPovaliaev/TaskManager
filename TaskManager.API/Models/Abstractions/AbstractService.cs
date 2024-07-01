using System;

namespace TaskManager.API.Models.Abstractions
{
    public abstract class AbstractService
    {
        protected bool DoAction(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
