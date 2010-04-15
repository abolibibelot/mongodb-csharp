﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MongoDB.Driver.Linq.Expressions
{
    internal class Nominator : MongoExpressionVisitor
    {
        private Func<Expression, bool> _predicate;
        private HashSet<Expression> _candidates;
        private bool _isBlocked;

        public Nominator(Func<Expression, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            _predicate = predicate;
        }

        public HashSet<Expression> Nominate(Expression expression)
        {
            _candidates = new HashSet<Expression>();
            _isBlocked = false;
            Visit(expression);
            return _candidates;
        }

        protected override Expression Visit(Expression expression)
        {
            if (expression != null)
            {
                var saveIsBlocked = _isBlocked;
                _isBlocked = false;
                base.Visit(expression);
                if (!_isBlocked)
                {
                    if (_predicate(expression))
                        _candidates.Add(expression);
                    else
                        _isBlocked = true;
                }
                _isBlocked |= saveIsBlocked;
            }
            return expression;
        }
    }
}