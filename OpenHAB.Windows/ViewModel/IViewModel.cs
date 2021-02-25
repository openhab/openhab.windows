namespace OpenHAB.Windows.ViewModel
{
    /// <summary>
    /// IViewModelModelProp.
    /// </summary>
    /// <typeparam name="T">Model type interface.</typeparam>
    public interface IViewModel<T>
    {
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        T Model
        {
            get;
            set;
        }
    }
}
