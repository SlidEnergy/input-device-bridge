namespace tser
{
    internal class Select62Handler
    {
        private InputSimulator sim;
        private Mover mover;

        public Select62Handler(InputSimulator sim)
        {
            this.sim = sim;
            mover = new Mover(sim);
        }

        public async Task Run(HandlerContext context)
        {
            await mover.MoveAndClick(457, 393, 579, 403);

            await mover.MoveAndClick(460, 471, 576, 488);


            await mover.MoveAndClick(614, 392, 727, 405);

            await mover.MoveAndClick(613, 471, 728, 485);


            await mover.MoveAndClick(768, 394, 884, 405);
            await mover.MoveAndClick(764, 445, 880, 458);
        }
    }
}
