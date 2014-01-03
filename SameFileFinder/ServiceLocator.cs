namespace SameFileFinder
{
    public static class ServiceLocator
    {
        private static readonly IServiceLocator m_Locator = new MefServiceLocator();
        public static IServiceLocator Current 
        {
            get
            {
                return m_Locator;
            }
        }
    }
}
