/*
   Copyright (c) 2018 Michał Wilczyński

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Threading.Tasks;
using Bureaucratize.Common.Core.Infrastructure.ResultMessages;

namespace Bureaucratize.Common.Core.Infrastructure.Common
{
    public interface ICommandHandler<TCommand> : ICommandHandler<TCommand, Nothing> where TCommand : ICommand
    {
    }

    public interface ICommandHandler<TCommand, TResult> : ICommandHandler where TCommand : ICommand
    {
        OperationResult<TResult> Handle(TCommand command);
    }

    public interface ICommandHandler
    {
    }

    public class Nothing
    {
    }
}
