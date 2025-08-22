using ClipShare.Core.IRepo;
using ClipShare.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipShare.DataAccess.Repo
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context context;

        public UnitOfWork(Context context)
        {
            this.context = context;
        }

        public IChannelRepo ChannelRepo => new ChannelRepo(context);
        public ICategoryRepo CategoryRepo => new CategoryRepo(context);
        public IVideoRepo VideoRepo => new VideoRepo(context);

        public async Task<bool> CompleteAsync()
        {
            var result = false;
            if (context.ChangeTracker.HasChanges())
            {
                result = await context.SaveChangesAsync() > 0;
            }
            return result;
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
