namespace tser
{
    internal class SellHandler
    {
        private InputSimulator sim;
        private Mover mover;

        public SellHandler(InputSimulator inputSimulator) 
        {
            sim = inputSimulator;
            mover = new Mover(sim);
        }

        public async Task Run()
        {
            var from = Cursor.Position;

            sim.LeftButtonClick();
            await Task.Delay(50);

            await mover.MoveAndClick(559, 626, 570, 639);

            await mover.MoveAndClick(844, 722, 926, 740);

            mover.MoveSmooth(from.X, from.Y);
        }
    }
}
