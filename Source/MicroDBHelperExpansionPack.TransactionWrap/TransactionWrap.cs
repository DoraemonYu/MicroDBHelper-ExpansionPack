using MicroDBHelpers;
using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MicroDBHelpers.ExpansionPack
{
    /// <summary>
    /// Wrap for MicroDBTransaction
    /// </summary>
    public class TransactionWrap : IDisposable
    {

        //--------Members----------

        #region Transaction Object

        /// <summary>
        /// Transaction Object
        /// </summary>
        readonly MicroDBTransaction transaction = null;

        #endregion

        #region Informations

        /// <summary>
        /// Level of Isolation (read only)
        /// </summary>
        public IsolationLevel IsolationLevel { get { return transaction.IsolationLevel; } }

        /// <summary>
        /// the Alias Name of Connection (read only)
        /// </summary>
        public string ConnectionAliasName { get { return transaction.ConnectionAliasName; } }

        #endregion

        #region Status

        /// <summary>
        /// Mark the status for MainDLL
        /// </summary>
        static bool IsMainDLLLoadedCorrect = false;

        #endregion


        //--------Control----------

        #region Static, Check Assembly

        static TransactionWrap()
        {
            //Check only once
            IsMainDLLLoadedCorrect = MicroDBHelperExpansionPack.Internals.AssemblyInspector.CheckAssembly();
        }

        static void CheckAssembly()
        {
            if (IsMainDLLLoadedCorrect == false)
            {
                const string ErrorMsg = @"Assemblies of current appDomain MUST at least one reference the [MicroDBHelper], then the [MicroDBHelperExpansionPack.TransactionWrap] will work properly.";

                System.Diagnostics.Trace.WriteLine("## [MicroDBHelperExpansionPack.TransactionWrap] " + ErrorMsg);
                throw new DllNotFoundException(ErrorMsg);
            }
        }

        #endregion


        #region ctor

        /// <summary>
        /// Hide default ctor
        /// </summary>
        private TransactionWrap(IsolationLevel IsolationLevel, string ConnectionAliasName = MicroDBHelper.ALIAS_NAME_DEFAULT)
        {
            transaction = MicroDBHelper.UseTransaction(IsolationLevel, ConnectionAliasName);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// mark as is it disposed now
        /// </summary>
        public bool IsDisposed { get; private set; }
      

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Release
                if (IsDisposed == false)
                    transaction.Dispose();
            }
            
            IsDisposed = true;
        }

        /// <summary>
        /// destruct
        /// </summary>
        ~TransactionWrap()
        {
            Dispose(false);
        }
       
        #endregion

        
        #region Implicit Conversion

        /// <summary>
        /// implicit conversion,  TransactionWrap -> MicroDBTransaction
        /// </summary>
        public static implicit operator MicroDBTransaction(TransactionWrap wrap)
        {
            if (Object.ReferenceEquals(wrap, null)) return null;

            return wrap.transaction;
        }

        #region Override CHECK, To avoid the "IF" check problem

        /// <summary>
        /// ==
        /// </summary>
        public static bool operator ==(TransactionWrap A, TransactionWrap B)
        {
            return Object.Equals(A, B);
        }
        /// <summary>
        /// !=
        /// </summary>
        public static bool operator !=(TransactionWrap A, TransactionWrap B)
        {
            return !Object.Equals(A, B);
        }
        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        /// <summary>
        /// GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion


        #endregion

        #region ToString()

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return transaction.ToString();
        }

        #endregion


        #region Get New Transaction Object

        /// <summary>
        /// Get new MicroDBTransaction Object with the Transaction support<para />
        /// ( when transaction is success，call MarkSuccess() method at last，then the transaction will automatically COMMIT; when some exception is happend then case the code NOT to call MarkSuccess() method,  the transaction will automatically Rollback )
        /// </summary>
        /// <param name="IsolationLevel">Level of Isolation</param>
        /// <param name="ConnectionAliasName">the Alias Name of Connection (if not pass name,it will use the DEFAULT name instead.)</param>
        public static TransactionWrap UseTransaction(IsolationLevel IsolationLevel, string ConnectionAliasName = MicroDBHelper.ALIAS_NAME_DEFAULT)
        {
            CheckAssembly();

            return new TransactionWrap(IsolationLevel, ConnectionAliasName);
        }

        #endregion

        #region Mark Result

        /// <summary>
        /// Mark this Transaction is Success ( before this MicroDBTransaction END,it will commit; otherwise, it wll rollback  )
        /// </summary>
        public void MarkSuccess()
        {
            if (transaction != null)
                this.transaction.MarkSuccess();
        }

        #endregion
    }

}
