using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//taken and adapted from Darklink's power GamePassCatalogBrowser plugin

namespace XCloudLibrary
{

    public class XCloudGame
    {
        public string BackgroundImageUrl { get; set; }
        public string Category { get; set; }
        public string[] Categories { get; set; }
        public string CoverImageUrl { get; set; }
        public string Description { get; set; }
        public string[] Developers { get; set; }
        public string IconUrl { get; set; }
        public string Name { get; set; }
        public string GameId { get; set; }
        public string[] Publishers { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string OriginalProductTitle { get; set; }
    }

    public partial class GameID
    {
        public string siglId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string requiresShuffling { get; set; }
        public string imageUrl { get; set; }
        public string id { get; set; }
    }

    public partial class CatalogData
    {
        public CatalogProduct[] Products { get; set; }
    }

    public partial class CatalogProduct
    {
        public DateTimeOffset LastModifiedDate { get; set; }
        public ProductLocalizedProperty[] LocalizedProperties { get; set; }
        public ProductMarketProperty[] MarketProperties { get; set; }
        public string ProductId { get; set; }
        public ProductProperties Properties { get; set; }
        public string ProductASchema { get; set; }
        public string ProductBSchema { get; set; }
    }

    public partial class ProductMarketProperty
    {
        public DateTimeOffset OriginalReleaseDate { get; set; }
        public RelatedProduct[] RelatedProducts { get; set; }
    }

    public partial class RelatedProduct
    {
        public string RelatedProductId { get; set; }
        public string RelationshipType { get; set; }
    }

    public partial class ProductLocalizedProperty
    {
        public string DeveloperName { get; set; }
        public string PublisherName { get; set; }
        public string PublisherWebsiteUri { get; set; }
        public object[] Franchises { get; set; }
        public Image[] Images { get; set; }
        public string ProductDescription { get; set; }
        public string ProductTitle { get; set; }
        public string ShortTitle { get; set; }
        public string SortTitle { get; set; }
        public string ShortDescription { get; set; }
        public string Language { get; set; }
        public Market[] Markets { get; set; }
    }

    public partial class ProductProperties
    {

        public Attribute[] Attributes { get; set; }
        public string Category { get; set; }
        public string[] Categories { get; set; }
        public string PackageFamilyName { get; set; }
        public string PackageIdentityName { get; set; }
        public string PublisherCertificateName { get; set; }
        public bool HasAddOns { get; set; }
    }

    public partial class Attribute
    {
        public string Name { get; set; }
    }

    public partial class Image
    {
        public long Height { get; set; }
        public long Width { get; set; }
        public ImagePurpose ImagePurpose { get; set; }
        public string Uri { get; set; }
    }

    public enum Market { Ad, Ae, Af, Ag, Ai, Al, Am, Ao, Aq, Ar, As, At, Au, Aw, Ax, Az, Ba, Bb, Bd, Be, Bf, Bg, Bh, Bi, Bj, Bl, Bm, Bn, Bo, Bq, Br, Bs, Bt, Bv, Bw, By, Bz, Ca, Cc, Cd, Cf, Cg, Ch, Ci, Ck, Cl, Cm, Cn, Co, Cr, Cv, Cw, Cx, Cy, Cz, De, Dj, Dk, Dm, Do, Dz, Ec, Ee, Eg, Eh, Er, Es, Et, Fi, Fj, Fk, Fm, Fo, Fr, Ga, Gb, Gd, Ge, Gf, Gg, Gh, Gi, Gl, Gm, Gn, Gp, Gq, Gr, Gs, Gt, Gu, Gw, Gy, Hk, Hm, Hn, Hr, Ht, Hu, Id, Ie, Il, Im, In, Io, Iq, Is, It, Je, Jm, Jo, Jp, Ke, Kg, Kh, Ki, Km, Kn, Kr, Kw, Ky, Kz, La, Lb, Lc, Li, Lk, Lr, Ls, Lt, Lu, Lv, Ly, Ma, Mc, Md, Me, Mf, Mg, Mh, Mk, Ml, Mm, Mn, Mo, Mp, Mq, Mr, Ms, Mt, Mu, Mv, Mw, Mx, My, Mz, Na, Nc, Ne, Neutral, Nf, Ng, Ni, Nl, No, Np, Nr, Nu, Nz, Om, Pa, Pe, Pf, Pg, Ph, Pk, Pl, Pm, Pn, Ps, Pt, Pw, Py, Qa, Re, Ro, Rs, Ru, Rw, Sa, Sb, Sc, Se, Sg, Sh, Si, Sj, Sk, Sl, Sm, Sn, So, Sr, St, Sv, Sx, Sz, Tc, Td, Tf, Tg, Th, Tj, Tk, Tl, Tm, Tn, To, Tr, Tt, Tv, Tw, Tz, Ua, Ug, Um, Us, Uy, Uz, Va, Vc, Ve, Vg, Vi, Vn, Vu, Wf, Ws, Ye, Yt, Za, Zm, Zw };

    public enum ImagePurpose { BoxArt, BrandedKeyArt, FeaturePromotionalSquareArt, Hero, Logo, Poster, Screenshot, SuperHeroArt, Tile, TitledHeroArt, Trailer };

}
