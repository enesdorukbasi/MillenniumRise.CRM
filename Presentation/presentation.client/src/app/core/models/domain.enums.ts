export enum SortingDirection { Ascending, Descending }
export enum CustomerType { Individual, Business }
export enum CustomerProductStatus { New, InProgress, FollowUpRequired, OnHold, Successful, Unsuccessful }
export enum InteractionChannel { Phone, Email, WhatsApp, SocialMedia, OnlineMeeting, InPersonMeeting, Other }
export enum InteractionStatus { Planned, Completed, NoResponse, Cancelled }
export enum InteractionOutcome { Positive, Neutral, Negative, FollowUpRequired }

export const customerTypeLabels: Record<CustomerType, string> = {
  [CustomerType.Individual]: 'Bireysel',
  [CustomerType.Business]: 'Kurumsal'
};

export const customerProductStatusLabels: Record<CustomerProductStatus, string> = {
  [CustomerProductStatus.New]: 'Yeni',
  [CustomerProductStatus.InProgress]: 'Devam ediyor',
  [CustomerProductStatus.FollowUpRequired]: 'Takip gerekli',
  [CustomerProductStatus.OnHold]: 'Beklemede',
  [CustomerProductStatus.Successful]: 'Başarılı',
  [CustomerProductStatus.Unsuccessful]: 'Başarısız'
};

export const interactionChannelLabels: Record<InteractionChannel, string> = {
  [InteractionChannel.Phone]: 'Telefon',
  [InteractionChannel.Email]: 'E-posta',
  [InteractionChannel.WhatsApp]: 'WhatsApp',
  [InteractionChannel.SocialMedia]: 'Sosyal medya',
  [InteractionChannel.OnlineMeeting]: 'Çevrim içi toplantı',
  [InteractionChannel.InPersonMeeting]: 'Yüz yüze toplantı',
  [InteractionChannel.Other]: 'Diğer'
};

export const interactionChannelIconClasses: Record<InteractionChannel, string> = {
  [InteractionChannel.Phone]: 'bi bi-telephone-fill',
  [InteractionChannel.Email]: 'bi bi-envelope-fill',
  [InteractionChannel.WhatsApp]: 'bi bi-whatsapp',
  [InteractionChannel.SocialMedia]: 'bi bi-chat-dots-fill',
  [InteractionChannel.OnlineMeeting]: 'bi bi-camera-video-fill',
  [InteractionChannel.InPersonMeeting]: 'bi bi-people-fill',
  [InteractionChannel.Other]: 'bi bi-three-dots'
};

export const interactionChannelMarkerClasses: Record<InteractionChannel, string> = {
  [InteractionChannel.Phone]: 'interaction-channel--phone',
  [InteractionChannel.Email]: 'interaction-channel--email',
  [InteractionChannel.WhatsApp]: 'interaction-channel--whatsapp',
  [InteractionChannel.SocialMedia]: 'interaction-channel--social',
  [InteractionChannel.OnlineMeeting]: 'interaction-channel--online-meeting',
  [InteractionChannel.InPersonMeeting]: 'interaction-channel--in-person',
  [InteractionChannel.Other]: 'interaction-channel--other'
};

export const interactionStatusLabels: Record<InteractionStatus, string> = {
  [InteractionStatus.Planned]: 'Planlandı',
  [InteractionStatus.Completed]: 'Tamamlandı',
  [InteractionStatus.NoResponse]: 'Yanıt alınamadı',
  [InteractionStatus.Cancelled]: 'İptal edildi'
};

export const interactionOutcomeLabels: Record<InteractionOutcome, string> = {
  [InteractionOutcome.Positive]: 'Olumlu',
  [InteractionOutcome.Neutral]: 'Nötr',
  [InteractionOutcome.Negative]: 'Olumsuz',
  [InteractionOutcome.FollowUpRequired]: 'Takip gerekli'
};
