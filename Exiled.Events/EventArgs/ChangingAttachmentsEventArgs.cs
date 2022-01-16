// -----------------------------------------------------------------------
// <copyright file="ChangingAttachmentsEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Structs;

    /// <summary>
    /// Contains all informations before changing item attachments.
    /// </summary>
    public class ChangingAttachmentsEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingAttachmentsEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="firearm"><inheritdoc cref="Firearm"/></param>
        /// <param name="oldIdentifiers"><inheritdoc cref="OldAttachmentIdentifiers"/></param>
        /// <param name="newIdentifiers"><inheritdoc cref="NewAttachmentIdentifiers"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingAttachmentsEventArgs(
            Player player,
            Firearm firearm,
            IEnumerable<AttachmentIdentifier> oldIdentifiers,
            IEnumerable<AttachmentIdentifier> newIdentifiers,
            bool isAllowed = true)
        {
            Player = player;
            Firearm = firearm;
            OldAttachmentIdentifiers = oldIdentifiers.ToArray();
            NewAttachmentIdentifiers = newIdentifiers.ToList();
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who's changing attachments.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm"/> which is being modified.
        /// </summary>
        public Firearm Firearm { get; }

        /// <summary>
        /// Gets the old <see cref="AttachmentIdentifier"/>.
        /// </summary>
        public AttachmentIdentifier[] OldAttachmentIdentifiers { get; }

        /// <summary>
        /// Gets or sets the new <see cref="AttachmentIdentifier"/>.
        /// </summary>
        public List<AttachmentIdentifier> NewAttachmentIdentifiers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the attachments can be changed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
