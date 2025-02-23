namespace StackTower.Code.Game.View.Informers
{
internal interface IInformer
{
    void Inform(string localizationKey);
    void Inform(string localizationKey, object value);
}
}